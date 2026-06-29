Imports System
Imports System.Threading.Tasks
Imports Microsoft.VisualBasic.ApplicationServices
Imports Opc.Ua
Imports Opc.Ua.Client
Public Class Form1
    Async Function Main() As Task(Of String)
        Dim serverUrl As String = "opc.tcp://localhost:53530/OPCUA/SimulationServer"
        Try
            Console.WriteLine("Configuring OPC UA Client Application...")
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


        Catch ex As Exception

        End Try
    End Function
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click

    End Sub
End Class
