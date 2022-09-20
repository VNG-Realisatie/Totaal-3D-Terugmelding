export default {

    regex : /https:\/\/t3d-./gm,

    get backend_base() {

        let url = window.location.origin;

        if(url.startsWith('http://localhost')) 
        {
            return "https://t3d-o-functions.azurewebsites.net/api";
        }

        if(url.startsWith('https://t3dstorage')) 
        {
            return "https://t3dbackend.azurewebsites.net/api";
        }
        
        
        let urlstart = this.getStart(url);        
        return `${urlstart}-functions.azurewebsites.net/api`;
    },
    get frontend_base() {
        
        let url = window.location.origin;

        if(url.startsWith('http://localhost')) 
        {
            return "https://t3d-o-cdn.azureedge.net";
            return "https://t3dstorage.z6.web.core.windows.net"
        }

        if(url.startsWith('https://t3dstorage')) 
        {
            return "https://t3dstorage.z6.web.core.windows.net";
        }

        let urlstart = this.getStart(url);
        return `${urlstart}-cdn.azureedge.net`;

    },
    getStart(url){              
        var matches = this.regex.exec(url);
        this.regex.lastIndex = 0; //reset regex
        return matches[0];
    }


}