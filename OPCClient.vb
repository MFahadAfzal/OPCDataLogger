
Imports System
Imports System.Diagnostics
Imports System.Threading
Imports System.Threading.Channels
Imports System.Threading.Tasks
Imports Opc.Ua
Imports Opc.Ua.Client
Imports Opc.Ua.Security.Certificates

Public Class OPCClient

    ' Handles all SQL Server reads/writes - OPCClient no longer talks to the
    ' database directly
    Private dataService As New DataService()

    ' Address of the Prosys OPC UA Simulation Server we connect to
    Private serverUrl As String = "opc.tcp://localhost:53530/OPCUA/SimulationServer"


    ' The live session/connection to the OPC UA server, created in Connect()
    Private opcSession As Session

    ' Reference to the form, stored so we can safely marshal notifications
    ' back to the UI thread (InvokeRequired / BeginInvoke) from inside monitor()
    Private uiForm As Form1
    Public Sub New(form As Form1)
        uiForm = form
    End Sub

    ' Maps tag name (e.g. "Counter") -> NodeId, populated by BrowseTags()
    Private tagNodes As New Dictionary(Of String, NodeId)

    ' Delegate wrapping monitor(), used both to subscribe to the Notification
    ' event and to re-invoke monitor() on the UI thread when needed
    Dim m_SessionNotification = New NotificationEventHandler(AddressOf monitor)

    ' Maps ClientHandle (assigned when a MonitoredItem is created) -> tag name,
    ' so incoming notifications (which only carry a handle) can be traced
    ' back to a specific tag
    Private tagHandles As New Dictionary(Of UInteger, String)

    ' Connects to the OPC UA server: configures the client application,
    ' sets up certificate stores, creates/loads the app's identity certificate,
    ' and establishes a session
    Public Async Function Connect() As Task
        Debug.WriteLine("Configuring OPC UA Client Application...")

        ' Describe our app to the OPC UA library - name, type, and unique ID
        Dim config = New ApplicationConfiguration()
        config.ApplicationName = "MyOPCUAClient"
        config.ApplicationType = ApplicationType.Client
        config.ApplicationUri = "urn:localhost:MyOPCDataLogger"

        ' Set up certificate stores - just telling the library where to find/save certificates
        Dim secConfig = New SecurityConfiguration()

        ' Our own app's identity certificate - stored in MachineDefault folder
        Dim cerIden = New CertificateIdentifier
        cerIden.StoreType = "Directory"
        cerIden.StorePath = "%CommonApplicationData%\OPC Foundation\CertificateStores\MachineDefault"
        secConfig.ApplicationCertificate = cerIden

        ' Certificates we explicitly trust regardless of who issued them
        Dim trustedPeer = New CertificateTrustList
        trustedPeer.StoreType = "Directory"
        trustedPeer.StorePath = "%CommonApplicationData%\OPC Foundation\CertificateStores\UA Applications"
        secConfig.TrustedPeerCertificates = trustedPeer

        ' Certificate authorities we trust - anything they signed we trust automatically
        Dim trustedIssues = New CertificateTrustList
        trustedIssues.StoreType = "Directory"
        trustedIssues.StorePath = "%CommonApplicationData%\OPC Foundation\CertificateStores\UA Issuers"
        secConfig.TrustedIssuerCertificates = trustedIssues

        ' Where failed/rejected certificates get dumped for inspection
        Dim rejectedCerts = New CertificateTrustList
        rejectedCerts.StoreType = "Directory"
        rejectedCerts.StorePath = "%CommonApplicationData%\OPC Foundation\CertificateStores\RejectedCertificates"
        secConfig.RejectedCertificateStore = rejectedCerts

        ' Skip strict certificate checking - fine for dev, turn off in production
        secConfig.AutoAcceptUntrustedCertificates = True

        config.SecurityConfiguration = secConfig

        ' Empty transport config - defaults are fine for OPC UA over TCP
        config.TransportConfigurations = New TransportConfigurationCollection()

        ' Give up on any operation that takes longer than 15 seconds
        config.TransportQuotas = New TransportQuotas
        config.TransportQuotas.OperationTimeout = 15000

        ' Drop the session if idle for 60 seconds
        config.ClientConfiguration = New ClientConfiguration
        config.ClientConfiguration.DefaultSessionTimeout = 60000

        ' Validate everything we just configured is correct for a client app
        Await config.ValidateAsync(ApplicationType.Client)

        ' Check if our app already has an identity certificate saved from a previous run
        Dim hasCert = Await config.SecurityConfiguration.ApplicationCertificate.FindAsync(True)

        ' If no certificate exists yet, create one and save it
        If hasCert Is Nothing Then
            Debug.WriteLine("Creating new application instance certificate...")
            Dim certBuilder As CertificateBuilder = CertificateFactory.CreateCertificate(config.ApplicationUri, config.ApplicationName, Nothing, Nothing)
            Dim clientCertificate = certBuilder.CreateForRSA()
            config.SecurityConfiguration.ApplicationCertificate.Certificate = clientCertificate
        End If

        ' Only bypass certificate validation if we've explicitly said to accept untrusted certs
        If config.SecurityConfiguration.AutoAcceptUntrustedCertificates Then
            AddHandler config.CertificateValidator.CertificateValidation,
                Sub(validator, e)
                    e.Accept = True
                End Sub
        End If

        ' Pick an endpoint on the server and open a session against it
        Dim endpoint = Await CoreClientUtils.SelectEndpointAsync(config, serverUrl, True, 5000)
        Dim EndpointConfig = EndpointConfiguration.Create(config)
        Dim SessionEndpoint = New ConfiguredEndpoint(Nothing, endpoint, EndpointConfig)

        Dim identity = New UserIdentity(New AnonymousIdentityToken())
        opcSession = Await Session.Create(config, SessionEndpoint, False, "MySession", 60000, identity, Nothing)
        Debug.WriteLine("Successfully connected to Prosys Server!")
    End Function

    ' Browses the server's node tree to find the Simulation folder and resolve
    ' the NodeId of each of the 7 simulation tags - no NodeIds are hardcoded
    Public Async Function BrowseTags() As Task(Of Dictionary(Of String, NodeId))
        ' Set up browser to traverse the OPC UA node tree
        Dim browser = New Browser(opcSession)
        ' Traverse down the tree - parent to child
        browser.BrowseDirection = BrowseDirection.Forward
        ' Only return folders, data values, and callable methods - filter out noise
        browser.NodeClassMask = CInt(NodeClass.Object) Or CInt(NodeClass.Variable) Or CInt(NodeClass.Method)
        ' Only follow parent-child relationships, not sideways connections like properties
        browser.ReferenceTypeId = ReferenceTypeIds.HierarchicalReferences
        ' Also follow subtypes like Organizes and HasComponent since HierarchicalReferences is never used directly
        browser.IncludeSubtypes = True

        ' Start browsing from the root Objects folder
        Dim nodeToBrowse = ObjectIds.ObjectsFolder
        Dim references = Await browser.BrowseAsync(nodeToBrowse)

        ' Find the "Simulation" folder among the top-level nodes
        Dim sim As NodeId = Nothing
        For Each item In references
            If item.DisplayName.Text = "Simulation" Then
                sim = ExpandedNodeId.ToNodeId(item.NodeId, opcSession.NamespaceUris)
            End If
        Next

        ' Browse inside the Simulation folder to find the individual tags
        nodeToBrowse = sim
        references = Await browser.BrowseAsync(nodeToBrowse)

        ' Store each tag's name -> NodeId mapping
        For Each item In references
            sim = ExpandedNodeId.ToNodeId(item.NodeId, opcSession.NamespaceUris)
            tagNodes(item.DisplayName.Text) = sim
        Next


        Return tagNodes
    End Function

    ' Creates one subscription and adds one MonitoredItem per tag in tagNodes,
    ' then hooks up the Notification event so monitor() runs whenever a
    ' subscribed value changes on the server
    Public Async Function Subscribe() As Task
        ' Build a subscription using the session's default settings as a template
        Dim Subscribing = New Subscription(opcSession.DefaultSubscription)
        opcSession.AddSubscription(Subscribing)

        ' Create one MonitoredItem per tag, watching each tag's NodeId
        For Each kvp In tagNodes
            Dim MonitorItem = New MonitoredItem(Subscribing.DefaultItem)
            MonitorItem.QueueSize = 1 ' only keep the latest value, discard older pending ones
            MonitorItem.StartNodeId = kvp.Value
            Subscribing.AddItem(MonitorItem)


            ' Record handle -> tag name now, while we still know which tag this is,
            ' so notifications (which only carry the handle) can be traced back later
            tagHandles(MonitorItem.ClientHandle) = kvp.Key
        Next

        ' Push the subscription and its monitored items to the server
        Subscribing.ChangesCompleted()
        Await Subscribing.CreateAsync()

        ' Start listening for live data-change notifications
        AddHandler opcSession.Notification, m_SessionNotification
    End Function

    ' Runs whenever the server sends a notification (a subscribed value changed).
    ' Always arrives on a background thread from the OPC UA library, so this
    ' method first marshals itself onto the UI thread before touching any controls
    Public Async Function monitor(session As Session, e As NotificationEventArgs) As Task
        ' If we're on a background thread, re-invoke this same method on the UI
        ' thread and stop executing here
        If uiForm.InvokeRequired Then
            uiForm.BeginInvoke(m_SessionNotification, session, e)
            Return
            ' Guard against the form's window handle no longer existing (e.g. closing)
        ElseIf Not uiForm.IsHandleCreated Then
            Return
        End If

        Try
            ' Process every value change included in this notification
            For Each change In e.NotificationMessage.GetDataChanges(False)
                ' Look up which tag this handle corresponds to
                Dim name = tagHandles(change.ClientHandle)

                ' Persist the change - DataService owns the actual insert/update logic
                dataService.LogChange(name, change.Value.WrappedValue.ToString(), change.Value.SourceTimestamp)

                ' Find the label on the form whose Name matches the tag name
                Dim foundLabel = CType(uiForm.Controls.Find(name, True)(0), Label)

                ' Update the label with the new value
                foundLabel.Text = name & ": " & change.Value.WrappedValue.ToString()

            Next
        Catch ex As Exception
            Debug.WriteLine("Error: " & ex.Message)
        End Try
    End Function
End Class