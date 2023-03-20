namespace HB.Match3.Behaviours
{
    public enum SwapResponse
    {
        Success,
        InvalidBlock,
        IgnoredBlockID,
        NotAdjacent,
        WontMatch,
        Restricted,
        InvalidDirection
    }
}