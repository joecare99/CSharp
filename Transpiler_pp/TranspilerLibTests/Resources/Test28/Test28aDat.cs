private void Test28a(bool b1, bool b2)
{
    if (b1)
    {
        {
            // Preserve this line comment while removing redundant wrappers.
            if (b2)
            {
                E();
            }
        }
    }
}
