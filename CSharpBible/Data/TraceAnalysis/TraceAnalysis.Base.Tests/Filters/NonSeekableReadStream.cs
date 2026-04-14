namespace TraceAnalysis.Base.Tests.Filters;

/// <summary>
/// Read-only stream wrapper that disables seeking to validate stream buffering behavior.
/// </summary>
public sealed class NonSeekableReadStream : Stream
{
    private readonly MemoryStream _innerStream;

    public NonSeekableReadStream(byte[] buffer)
    {
        _innerStream = new MemoryStream(buffer);
    }

    public override bool CanRead => true;

    public override bool CanSeek => false;

    public override bool CanWrite => false;

    public override long Length => _innerStream.Length;

    public override long Position
    {
        get => _innerStream.Position;
        set => throw new NotSupportedException();
    }

    public override void Flush()
    {
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        return _innerStream.Read(buffer, offset, count);
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        throw new NotSupportedException();
    }

    public override void SetLength(long value)
    {
        throw new NotSupportedException();
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        throw new NotSupportedException();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
            _innerStream.Dispose();

        base.Dispose(disposing);
    }
}
