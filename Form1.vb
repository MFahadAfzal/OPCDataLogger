Imports System
Imports System.Diagnostics
Imports System.ComponentModel.DataAnnotations
Imports System.Net
Imports System.Security.Cryptography.X509Certificates
Imports System.Security.Principal
Imports System.Threading.Tasks
Imports Microsoft.VisualBasic.ApplicationServices
Imports Opc.Ua
Imports Opc.Ua.Client
Imports Opc.Ua.LoggerUtils
Imports Opc.Ua.Security.Certificates
Public Class Form1
    Async Function Main() As Task
        Dim serverUrl As String = "opc.tcp://localhost:53530/OPCUA/SimulationServer"
        Try
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

            Dim endpoint = Await CoreClientUtils.SelectEndpointAsync(config, serverUrl, True, 5000)
            Dim EndpointConfig = EndpointConfiguration.Create(config)
            Dim SessionEndpoint = New ConfiguredEndpoint(Nothing, endpoint, EndpointConfig)

            Dim identity = New UserIdentity(New AnonymousIdentityToken())
            Dim opcSession = Await Session.Create(config, SessionEndpoint, False, "MySession", 60000, identity, Nothing)
            Debug.WriteLine("Successfully connected to Prosys Server!")


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

            Dim nodeToBrowse = ObjectIds.ObjectsFolder
            Dim references = Await Browser.BrowseAsync(nodeToBrowse)

            Dim sim = Nothing
            For Each item In references
                If item.DisplayName.Text = "Simulation" Then
                    sim = ExpandedNodeId.ToNodeId(item.NodeId, opcSession.NamespaceUris)
                End If
            Next

            nodeToBrowse = sim
            references = Await browser.BrowseAsync(nodeToBrowse)

            Dim tagNodes As New Dictionary(Of String, NodeId)
            For Each item In references
                sim = ExpandedNodeId.ToNodeId(item.NodeId, opcSession.NamespaceUris)
                tagNodes(item.DisplayName.Text) = sim
            Next

            For Each kvp In tagNodes
                Debug.WriteLine(kvp.Key & " -> " & kvp.Value.ToString())
            Next



        Catch ex As Exception
            Debug.WriteLine("Error: " & ex.Message)
        End Try
    End Function
    Private Async Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click

    End Sub

    Private Async Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Await Main()
    End Sub

    Private Sub CheckedListBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles CheckedListBox1.SelectedIndexChanged

    End Sub

    Private Sub Label3_Click(sender As Object, e As EventArgs) Handles Label3.Click

    End Sub
End Class
