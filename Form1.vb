Imports System
Imports System.ComponentModel.DataAnnotations
Imports System.Security.Cryptography.X509Certificates
Imports System.Threading.Tasks
Imports Microsoft.VisualBasic.ApplicationServices
Imports Opc.Ua
Imports Opc.Ua.Client
Imports Opc.Ua.LoggerUtils
Public Class Form1
    Async Function Main() As Task(Of String)
        Dim serverUrl As String = "opc.tcp://localhost:53530/OPCUA/SimulationServer"
        Try
            Console.WriteLine("Configuring OPC UA Client Application...")

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
                Console.WriteLine("Creating new application instance certificate...")
                Dim clientCertificate As X509Certificate2 = CertificateFactory.CreateCertificate(
                config.ApplicationUri, config.ApplicationName, Nothing, Nothing)

                config.SecurityConfiguration.ApplicationCertificate.Certificate = clientCertificate
            End If

            ' Only bypass certificate validation if we've explicitly said to accept untrusted certs
            If config.SecurityConfiguration.AutoAcceptUntrustedCertificates Then
                AddHandler config.CertificateValidator.CertificateValidation,
                    Sub(validator, e)
                        e.Accept = True
                    End Sub
            End If




        Catch ex As Exception

        End Try
    End Function
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click

    End Sub
End Class
