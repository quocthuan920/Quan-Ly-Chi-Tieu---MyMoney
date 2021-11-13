package crc643aca110cad7896f8;


public class ModalContentPageRenderer
	extends crc643f46942d9dd1fff9.PageRenderer
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"";
		mono.android.Runtime.register ("MyMoney.Droid.Renderer.ModalContentPageRenderer, MyMoney.Android", ModalContentPageRenderer.class, __md_methods);
	}


	public ModalContentPageRenderer (android.content.Context p0, android.util.AttributeSet p1, int p2)
	{
		super (p0, p1, p2);
		if (getClass () == ModalContentPageRenderer.class)
			mono.android.TypeManager.Activate ("MyMoney.Droid.Renderer.ModalContentPageRenderer, MyMoney.Android", "Android.Content.Context, Mono.Android:Android.Util.IAttributeSet, Mono.Android:System.Int32, mscorlib", this, new java.lang.Object[] { p0, p1, p2 });
	}


	public ModalContentPageRenderer (android.content.Context p0, android.util.AttributeSet p1)
	{
		super (p0, p1);
		if (getClass () == ModalContentPageRenderer.class)
			mono.android.TypeManager.Activate ("MyMoney.Droid.Renderer.ModalContentPageRenderer, MyMoney.Android", "Android.Content.Context, Mono.Android:Android.Util.IAttributeSet, Mono.Android", this, new java.lang.Object[] { p0, p1 });
	}


	public ModalContentPageRenderer (android.content.Context p0)
	{
		super (p0);
		if (getClass () == ModalContentPageRenderer.class)
			mono.android.TypeManager.Activate ("MyMoney.Droid.Renderer.ModalContentPageRenderer, MyMoney.Android", "Android.Content.Context, Mono.Android", this, new java.lang.Object[] { p0 });
	}

	private java.util.ArrayList refList;
	public void monodroidAddReference (java.lang.Object obj)
	{
		if (refList == null)
			refList = new java.util.ArrayList ();
		refList.add (obj);
	}

	public void monodroidClearReferences ()
	{
		if (refList != null)
			refList.clear ();
	}
}
