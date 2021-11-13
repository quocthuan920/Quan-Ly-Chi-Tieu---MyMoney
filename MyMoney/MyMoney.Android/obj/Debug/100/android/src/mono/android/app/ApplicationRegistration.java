package mono.android.app;

public class ApplicationRegistration {

	public static void registerApplications ()
	{
				// Application and Instrumentation ACWs must be registered first.
		mono.android.Runtime.register ("MyMoney.Droid.MainApplication, MyMoney.Android, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", crc64319508858aeac2a5.MainApplication.class, crc64319508858aeac2a5.MainApplication.__md_methods);
		
	}
}
