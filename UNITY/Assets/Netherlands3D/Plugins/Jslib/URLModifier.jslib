mergeInto(LibraryManager.library,
{
    Unity_URLModifier_SetSessionIdInURL : function(string sessionId)
    {
        window.history.pushState({"sessionID": sessionId},"","?sessionId="+sessionId);
    }
});