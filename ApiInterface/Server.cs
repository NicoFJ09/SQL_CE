using System.Net.Sockets;
using System.Net;
using System.Text;
using ApiInterface.InternalModels;
using System.Text.Json;
using ApiInterface.Exceptions;
using ApiInterface.Processors;
using ApiInterface.Models;
using System.Runtime.InteropServices;

namespace ApiInterface
{
    public class Server
    {
        private static IPEndPoint serverEndPoint = new(IPAddress.Loopback, 11000);
        private static int supportedParallelConnections = 1;

        public static async Task Start()
        { 
            using Socket listener =new(serverEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            listener.Bind(serverEndPoint);
            listener.Listen(supportedParallelConnections);
            Console.WriteLine($"Server ready at {serverEndPoint.ToString()}");

            while (true)
            {
                var handler = await listener.AcceptAsync();
                try
                {
                    Console.WriteLine("Client connected.");
                    var rawMessage = GetMessage(handler);
                    Console.WriteLine($"Raw message received: {rawMessage}");

                    if (string.IsNullOrWhiteSpace(rawMessage))
                    {
                        Console.WriteLine("Empty or invalid message received.");
                        await SendErrorResponse("Empty or invalid message", handler);
                    }

                    var requestObject = ConvertToRequestObject(rawMessage);
                    Console.WriteLine($"Request object created: {JsonSerializer.Serialize(requestObject)}");

                    var response = ProcessRequest(requestObject);
                    Console.WriteLine("Response processed, sending response back.");
                    SendResponse(ProcessRequest(requestObject), handler);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error is:{ex}");
                    await SendErrorResponse("Unknown exception", handler);
                }
                finally
                {
                    handler.Close();
                }
            }
        }

        private static string GetMessage(Socket handler)
        {
            using (NetworkStream stream = new NetworkStream(handler))
            using (StreamReader reader = new StreamReader(stream))
            {
                // Console.WriteLine($"Received: {reader.ReadLine()}");
                // return reader.ReadLine() ?? String.Empty;
                string messagee = reader.ReadLine() ?? String.Empty;

                Console.WriteLine($"Received: {messagee}");
                return messagee;
            }
        }

        private static Request ConvertToRequestObject(string rawMessage)
        {
            return JsonSerializer.Deserialize<Request>(rawMessage) ?? throw new InvalidRequestException();
        }

        private static Response ProcessRequest(Request requestObject)
        {
            var processor = ProcessorFactory.Create(requestObject);
            return processor.Process();
        }

        private static void SendResponse(Response response, Socket handler)
        {
            using (NetworkStream stream = new NetworkStream(handler))
            using (StreamWriter writer = new StreamWriter(stream))
            {
                Console.WriteLine($"Sendresponse: {JsonSerializer.Serialize(response)}");
                writer.WriteLine(JsonSerializer.Serialize(response));
            }
        }

        private static Task SendErrorResponse(string reason, Socket handler)
        {
            throw new NotImplementedException();
        }

    }
}
