namespace EcommerceMVC.Helpers
{
    public static class MySetting
    {
        //chứa các hằng code
        public static string CART_KEY = "MYCART";//hằng code lưu trữ session cart
        public static string CLAIM_CUSTOMERID = "CustomerID";
    }

    public  class PaymentType
    {
        public static string COD = "COD";
		public static string Paypal = "Paypal";
	}
}
