private void Test24c(bool b1, bool b2)
{
    if (b1)
    {
        if (b2)
        {
            goto End;
        }
    }
End:
    SomeFunction();
}
