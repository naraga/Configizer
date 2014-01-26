namespace ConfigizerLib
{
    public class DummyConvertibleToAnything
    {
        public static implicit operator string(DummyConvertibleToAnything x)
        {
            return string.Empty;
        }

        public static implicit operator int(DummyConvertibleToAnything x)
        {
            return 42;
        }
    }
}