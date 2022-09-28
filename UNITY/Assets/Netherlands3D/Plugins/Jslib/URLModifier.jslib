mergeInto(LibraryManager.library, {
    Unity_URLModifier_SetSessionIdInURL: function(sessionId)
    {
        sessionId = Pointer_stringify(sessionId);
        console.log(sessionId);
        window.history.pushState({"sessionId": sessionId},"","?sessionId="+sessionId);
    }
});