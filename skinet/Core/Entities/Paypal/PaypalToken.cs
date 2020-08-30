namespace Core.Entities.Paypal
{
  public class PaypalToken
  {
    public string scope { get; set; }
    public string Access_Token { get; set; }
    public string Token_Type { get; set; }
    public string App_Id { get; set; }
    public int Expires_in { get; set; }
    public string Nonce { get; set; }
  }
}