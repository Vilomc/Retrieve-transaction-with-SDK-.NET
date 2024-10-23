using System;
using System.IO;
using com.clover.remotepay.sdk;
using com.clover.remotepay.transport;

namespace CloverRetrievePaymentNote

{
    class Program
    {
        public static ICloverConnector cloverConnector;
        public static string authToken = null; // Inicialmente nulo, se obtiene tras el emparejamiento
        public static bool isDeviceReady = false; // Indicador para saber si el dispositivo está listo

        static void Main()
        {
            // Inicializa la conexión sin un authToken predefinido
            InitializeCloverConnection();

            // Espera a que el dispositivo esté listo antes de recuperar el pago
            Console.WriteLine("Presiona cualquier tecla para recuperar el pago...");
            Console.ReadKey();

            // Aquí debes proporcionar el paymentId que deseas recuperar
            string paymentId = "691008941729258738299"; // Cambia esto por el ID real de tu pago
            RetrievePayment(paymentId);

            Console.ReadKey();
        }

        public static void InitializeCloverConnection()
        {
            Console.WriteLine("Iniciando conexión con Clover...");
            string deviceEndpoint = "ws://192.168.0.151:12345/remote_pay";
            string remoteApplicationID = "SWDEFOTWBD7XT.9MGLGMDLSYWTV";
            string posName = "Pruebas Retrieve info Note transaccion";
            string serialNumber = "No puede ir vacio pero acepta cualquier cosa";

            var websocketConfiguration = new WebSocketCloverDeviceConfiguration(
                deviceEndpoint,
                remoteApplicationID,
                true,
                1,
                posName,
                serialNumber,
                null,
                OnPairingCode,
                OnPairingSuccess,
                OnPairingState
            );
            cloverConnector = CloverConnectorFactory.createICloverConnector(websocketConfiguration);
            cloverConnector.AddCloverConnectorListener(new CustomCloverConnectorListener());
            cloverConnector.InitializeConnection();
        }
        public static void RetrievePayment(string paymentId)
        {
            Console.WriteLine($"Recuperando el pago con ID: {paymentId}");

            // Crear la solicitud de recuperación de pago
            var retrievePaymentRequest = new RetrievePaymentRequest
            {
                externalPaymentId = paymentId // Usar el ID de pago que deseas recuperar
            };

            // Enviar la solicitud de recuperación
            try
            {
                cloverConnector.RetrievePayment(retrievePaymentRequest);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al recuperar el pago: {ex.Message}");
            }
        }

        public static void OnPairingCode(string pairingCode)
        {
            Console.WriteLine("Código de emparejamiento: " + pairingCode);
        }

        public static void OnPairingSuccess(string pairingAuthToken)
        {
            Console.WriteLine("Emparejamiento exitoso. Token: " + pairingAuthToken);

            // Almacena el token de emparejamiento para su uso posterior
            authToken = pairingAuthToken;
        }

        public static void OnPairingState(string state, string message)
        {
            Console.WriteLine($"Estado de emparejamiento: {state} - {message}");
        }

      
        public class CustomCloverConnectorListener : ICloverConnectorListener
        {
            public void OnDeviceReady(MerchantInfo merchantInfo)
            {
                Console.WriteLine("Dispositivo listo para procesar transacciones.");
                Program.isDeviceReady = true; // Marca el dispositivo como listo
            }

            public void OnDeviceConnected()
            {
                Console.WriteLine("Dispositivo conectado.");
            }

            public void OnDeviceDisconnected()
            {
                Console.WriteLine("Dispositivo desconectado.");
                Program.isDeviceReady = false; // Marca el dispositivo como no listo
            }

            public void OnRetrievePaymentResponse(RetrievePaymentResponse response)
            {
                if (response.QueryStatus == QueryStatus.FOUND)
                {
                    Console.WriteLine($"Pago recuperado con éxito. ID de pago: {response.Payment.id}");
                    Console.WriteLine($"Monto: {response.Payment.amount}");
                    Console.WriteLine($"Estado de la transacción: {response.Payment.note}");
                }
                else
                {
                    Console.WriteLine($"Error al recuperar el pago. Estado de la consulta: {response.QueryStatus}");
                }
            }

            // Implementación de métodos adicionales de ICloverConnectorListener que se requieren
            public void OnDeviceActivityStart(CloverDeviceEvent deviceEvent) { }
            public void OnDeviceActivityEnd(CloverDeviceEvent deviceEvent) { }
            public void OnDeviceError(CloverDeviceErrorEvent deviceErrorEvent) { }
            public void OnIncrementPreAuthResponse(IncrementPreAuthResponse response) { }
            public void OnVoidPaymentRefundResponse(VoidPaymentRefundResponse response) { }
            public void OnTipAdded(TipAddedMessage message) { }
            public void OnVaultCardResponse(VaultCardResponse response) { }
            public void OnReadCardDataResponse(ReadCardDataResponse response) { }
            public void OnPrintManualRefundReceipt(PrintManualRefundReceiptMessage message) { }
            public void OnPrintManualRefundDeclineReceipt(PrintManualRefundDeclineReceiptMessage message) { }
            public void OnPrintPaymentReceipt(PrintPaymentReceiptMessage message) { }
            public void OnPrintPaymentDeclineReceipt(PrintPaymentDeclineReceiptMessage message) { }
            public void OnPrintPaymentMerchantCopyReceipt(PrintPaymentMerchantCopyReceiptMessage message) { }
            public void OnPrintRefundPaymentReceipt(PrintRefundPaymentReceiptMessage message) { }
            public void OnPrintJobStatusResponse(PrintJobStatusResponse response) { }
            public void OnRetrievePrintersResponse(RetrievePrintersResponse response) { }
            public void OnCustomActivityResponse(CustomActivityResponse response) { }
            public void OnRetrieveDeviceStatusResponse(RetrieveDeviceStatusResponse response) { }
            public void OnMessageFromActivity(MessageFromActivity message) { }
            public void OnResetDeviceResponse(ResetDeviceResponse response) { }
            public void OnPrintJobStatusRequest(PrintJobStatusRequest request) { }
            public void OnDisplayReceiptOptionsResponse(DisplayReceiptOptionsResponse response) { }
            public void OnInvalidStateTransitionResponse(InvalidStateTransitionNotification response) { }
            public void OnCustomerProvidedData(CustomerProvidedDataEvent eventData) { }
            public void OnPreAuthResponse(PreAuthResponse response) { }
            public void OnAuthResponse(AuthResponse response) { }
            public void OnsaleResponse(SaleResponse response) { }

            public void OnTipAdjustAuthResponse(TipAdjustAuthResponse response) { }
            public void OnCapturePreAuthResponse(CapturePreAuthResponse response) { }
            public void OnVerifySignatureRequest(VerifySignatureRequest request) { }
            public void OnConfirmPaymentRequest(ConfirmPaymentRequest response) { }
            public void OnCloseoutResponse(CloseoutResponse response) { }
            public void OnManualRefundResponse(ManualRefundResponse response) { }
            public void OnRefundPaymentResponse(RefundPaymentResponse response) { }
            public void OnVoidPaymentResponse(VoidPaymentResponse response) { }
            public void OnRetrievePendingPaymentsResponse(RetrievePendingPaymentsResponse response) { }

            public void OnSaleResponse(SaleResponse response)
            {
                throw new NotImplementedException();
            }
        }
    }
}
