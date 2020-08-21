namespace Lusid.FinDataEx.Vendor.Dl
{
 
    /// <summary>
    ///  Supported DL request types.
    /// </summary>
    public enum DlRequestType
    {
        /// <summary> Request type for retrieving either price or security reference data</summary>
        Prices,
        /// <summary> Request type for retrieving corporate actions data</summary>
        CorpActions
    }

    /// <summary>
    /// Supported corporate action types that can be retrieved from DL.
    /// 
    ///  TODO need to integrate these types into DL request processing instead of hard coded strings. Awaiting
    ///  TODO more info on corp action types.
    /// </summary>
    public enum DlCorpActionType
    {
        StockSplt,
        DvdStock,
        DvdCash
    }
}