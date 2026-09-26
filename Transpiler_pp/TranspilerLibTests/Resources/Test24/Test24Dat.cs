private void Test24(bool b1, bool b2)
{
    if (b1)
    {
        // This branch note belongs to the merged condition body.
        if (b2)
        {
            SomeFunction();
        }
    }
    NoElseFunction();
}
