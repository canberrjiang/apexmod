using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using PayPalCheckoutSdk.Core;
using PayPalHttp;

namespace Infrastructure.Config
{
  public class PaypalConfig
  {
    public static PayPalEnvironment environment()
    {
      return new SandboxEnvironment(
        "AUI1amXzeRQTLnD_93jiqM4FrPjolgChfFVCd7QVMZH_NO7z8mQdVivSUOGSaWgMX62Rzp9cuurdFSVl",
        "ENbDpNXmYdeXyJGkxqpBvwxmrZ9zu0sC_pPouFHeyN3DZ8BkfBjyKQ1M4QGFZc69VQqdyBDq-RqsUnH0");
    }

    public static HttpClient client()
    {
      return new PayPalHttpClient(environment());
    }

    public static HttpClient client(string refreshToken)
    {
      return new PayPalHttpClient(environment(), refreshToken);
    }

    public static string ObjectToJSONString(object serializableObject)
    {
      MemoryStream memoryStream = new MemoryStream();
      var writer = JsonReaderWriterFactory.CreateJsonWriter(
                  memoryStream, Encoding.UTF8, true, true, "  ");
      DataContractJsonSerializer ser = new DataContractJsonSerializer(serializableObject.GetType(), new DataContractJsonSerializerSettings { UseSimpleDictionaryFormat = true });
      ser.WriteObject(writer, serializableObject);
      memoryStream.Position = 0;
      StreamReader sr = new StreamReader(memoryStream);
      return sr.ReadToEnd();
    }

  }
}