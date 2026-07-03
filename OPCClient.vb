Imports System
Imports System.Diagnostics
Imports System.Threading
Imports System.Threading.Channels
Imports System.Threading.Tasks
Imports Opc.Ua
Imports Opc.Ua.Client
Imports Opc.Ua.Security.Certificates

Public Class OPCClient

    Private serverUrl As String = "opc.tcp://localhost:53530/OPCUA/SimulationServer"
    Private opcSession As Session

    Private uiForm As Form1
    Public Sub New(form As Form1)
        uiForm = form
    End Sub

    Private tagNodes As New Dictionary(Of String, NodeId)
    Dim m_SessionNotification = New NotificationEventHandler(AddressOf monitor)
    Private tagHandles As New Dictionary(Of UInteger, String)
    Public Async Function Connect() As Task
        Debug.WriteLine("Configuring OPC UA Client Application...")

        Dim config = New ApplicationConfiguration()
        config.ApplicationName = "MyOPCUAClient"
        config.ApplicationType = ApplicationType.Client
        config.ApplicationUri = "urn:localhost:MyOPCDataLogger"

        Dim secConfig = New SecurityConfiguration()

        Dim cerIden = New CertificateIdentifier
        cerIden.StoreType = "Directory"
        cerIden.StorePath = "%CommonApplicationData%\OPC Foundation\CertificateStores\MachineDefault"
        secConfig.ApplicationCertificate = cerIden

        Dim trustedPeer = New CertificateTrustList
        trustedPeer.StoreType = "Directory"
        trustedPeer.StorePath = "%CommonApplicationData%\OPC Foundation\CertificateStores\UA Applications"
        secConfig.TrustedPeerCertificates = trustedPeer

        Dim trustedIssues = New CertificateTrustList
        trustedIssues.StoreType = "Directory"
        trustedIssues.StorePath = "%CommonApplicationData%\OPC Foundation\CertificateStores\UA Issuers"
        secConfig.TrustedIssuerCertificates = trustedIssues

        Dim rejectedCerts = New CertificateTrustList
        rejectedCerts.StoreType = "Directory"
        rejectedCerts.StorePath = "%CommonApplicationData%\OPC Foundation\CertificateStores\RejectedCertificates"
        secConfig.RejectedCertificateStore = rejectedCerts

        secConfig.AutoAcceptUntrustedCertificates = True

        config.SecurityConfiguration = secConfig
        config.TransportConfigurations = New TransportConfigurationCollection()

        config.TransportQuotas = New TransportQuotas
        config.TransportQuotas.OperationTimeout = 15000

        config.ClientConfiguration = New ClientConfiguration
        config.ClientConfiguration.DefaultSessionTimeout = 60000

        Await config.ValidateAsync(ApplicationType.Client)

        Dim hasCert = Await config.SecurityConfiguration.ApplicationCertificate.FindAsync(True)

        If hasCert Is Nothing Then
            Debug.WriteLine("Creating new application instance certificate...")
            Dim certBuilder As CertificateBuilder = CertificateFactory.CreateCertificate(config.ApplicationUri, config.ApplicationName, Nothing, Nothing)
            Dim clientCertificate = certBuilder.CreateForRSA()
            config.SecurityConfiguration.ApplicationCertificate.Certificate = clientCertificate
        End If

        If config.SecurityConfiguration.AutoAcceptUntrustedCertificates Then
            AddHandler config.CertificateValidator.CertificateValidation,
                Sub(validator, e)
                    e.Accept = True
                End Sub
        End If

        Dim endpoint = Await CoreClientUtils.SelectEndpointAsync(config, serverUrl, True, 5000)
        Dim EndpointConfig = EndpointConfiguration.Create(config)
        Dim SessionEndpoint = New ConfiguredEndpoint(Nothing, endpoint, EndpointConfig)

        Dim identity = New UserIdentity(New AnonymousIdentityToken())
        opcSession = Await Session.Create(config, SessionEndpoint, False, "MySession", 60000, identity, Nothing)
        Debug.WriteLine("Successfully connected to Prosys Server!")
    End Function

    Public Async Function BrowseTags() As Task(Of Dictionary(Of String, NodeId))
        Dim browser = New Browser(opcSession)
        browser.BrowseDirection = BrowseDirection.Forward
        browser.NodeClassMask = CInt(NodeClass.Object) Or CInt(NodeClass.Variable) Or CInt(NodeClass.Method)
        browser.ReferenceTypeId = ReferenceTypeIds.HierarchicalReferences
        browser.IncludeSubtypes = True

        Dim nodeToBrowse = ObjectIds.ObjectsFolder
        Dim references = Await browser.BrowseAsync(nodeToBrowse)

        Dim sim As NodeId = Nothing
        For Each item In references
            If item.DisplayName.Text = "Simulation" Then
                sim = ExpandedNodeId.ToNodeId(item.NodeId, opcSession.NamespaceUris)
            End If
        Next

        nodeToBrowse = sim
        references = Await browser.BrowseAsync(nodeToBrowse)

        For Each item In references
            sim = ExpandedNodeId.ToNodeId(item.NodeId, opcSession.NamespaceUris)
            tagNodes(item.DisplayName.Text) = sim
        Next

        For Each kvp In tagNodes
            Debug.WriteLine(kvp.Key & " -> " & kvp.Value.ToString())
        Next

        Return tagNodes
    End Function

    Public Async Function Subscribe() As Task
        Dim Subscribing = New Subscription(opcSession.DefaultSubscription)
        opcSession.AddSubscription(Subscribing)
        For Each kvp In tagNodes
            Dim MonitorItem = New MonitoredItem(Subscribing.DefaultItem)
            MonitorItem.QueueSize = 1
            MonitorItem.StartNodeId = kvp.Value
            Subscribing.AddItem(MonitorItem)
            Debug.WriteLine(MonitorItem.ClientHandle)
            tagHandles(MonitorItem.ClientHandle) = kvp.Key
        Next
        Subscribing.ChangesCompleted()
        Await Subscribing.CreateAsync()
        AddHandler opcSession.Notification, m_SessionNotification


    End Function

    Public Async Function monitor(session As Session, e As NotificationEventArgs) As Task
        If uiForm.InvokeRequired Then
            uiForm.BeginInvoke(m_SessionNotification, session, e)
            Return
        ElseIf Not uiForm.IsHandleCreated Then
            Return
        End If

        Try
            For Each change In e.NotificationMessage.GetDataChanges(False)
                Dim name = tagHandles(change.ClientHandle)
                Dim foundLabel = CType(uiForm.Controls.Find(name, True)(0), Label)
                foundLabel.Text = name & ": " & change.Value.WrappedValue.ToString()
                Debug.WriteLine(change.Value.SourceTimestamp)
                Debug.WriteLine(change.Value.ServerTimestamp)
            Next
        Catch ex As Exception
            Debug.WriteLine("Error: " & ex.Message)
        End Try
    End Function
End Class