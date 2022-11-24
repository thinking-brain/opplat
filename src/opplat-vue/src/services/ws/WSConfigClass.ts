import ReconnectingWebSocket from 'reconnecting-websocket';
// Todo:Importante! Tratar de pasar las los proveedores de WS al Modelo tambien
export default class WSConfigClass {

    public name: string = '';
    public headers: object = {};
    public url: object = {};
    public readonly WSInstance = ReconnectingWebSocket;

    public setName(name: string) {
        this.name = name;
    }

    public getName(): string {
        return this.name;
    }

    public urlList(url: object) {
        this.url = url;
    }

    public setHeaders(headers: object) {
        this.headers = headers;
    }

    public makeRequest(url, method, data, id) {
        id = localStorage.getItem('localToken');
        const {ws_url, protocols, options} = this.genOptionObject(url, method, data, id);
        return new this.WSInstance(ws_url, protocols, options);
    }

    public getHeaders(): object {
        return this.headers;
    }

    public getUrlList(): object {
        return this.url;
    }

    private genOptionObject(url: string, method, data, id: string | number = '') {
        const object = {
            ws_url: process.env.VUE_APP_WS_URL + this.getName() + '/' + url + '/' + id + '/',
            protocols: [],
            options: {},
        };
        return object;
    }
}

