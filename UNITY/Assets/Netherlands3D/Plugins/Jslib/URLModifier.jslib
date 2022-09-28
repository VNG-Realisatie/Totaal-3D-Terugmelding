mergeInto(LibraryManager.library, {
    Unity_URLModifier_SetSessionIdInURL: function(sessionId)
    {
        console.log(sessionId);
        window.history.pushState({"sessionId": sessionId},"","?sessionId="+sessionId);
    }
});