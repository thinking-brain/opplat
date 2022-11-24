import { AxiosInstance, Method } from "axios";
import Axios, { AxiosResponse } from "axios";
import axios from "./AxiosConfig";
import { WSMethod } from "@/services/repos/IRepository";

export default class AxiosConfigClass {
  public name: string = "";
  public headers: object = {};
  public url: object = {};
  public axiosInstance: AxiosInstance = axios;

  public setAxiosInstance(baseURL: string | undefined) {
    const instance: AxiosInstance = Axios.create({
      baseURL: baseURL,
    });
    this.axiosInstance = instance;
  }

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

  public async makeRequest(
    method: Method | WSMethod,
    data,
    url: string = "",
    id: string | number = "",
    providedHeaders = {} as { [name: string]: any }
  ) {
    const options = this.genOptionObject(
      method,
      data,
      url,
      id,
      providedHeaders
    );
    const request = await this.axiosInstance({
      ...options,
      withCredentials: true,
    });
    return request.data;
  }

  public getHeaders(): object {
    return this.headers;
  }

  public getUrlList(): object {
    return this.url;
  }

  private getParams(data) {
    let finalParams: string = "?";
    if (Object.keys(data).length) {
      Object.keys(data).forEach((key, index) => {
        finalParams += `${key}=${data[key]}${
          index + 1 !== Object.keys(data).length ? "&" : ""
        }`;
      });
      return finalParams;
    }
    return "";
  }

  private genOptionObject(
    method: Method | WSMethod,
    data,
    url: string,
    id: string | number,
    providedHeaders = {} as { [name: string]: any }
  ): any {
    let urlFinal = "";
    const urlId = id ? id + "/" : "";
    const urlStr = url ? "/" + url : "";
    // const dataParams = method === 'GET' || method === 'get' ? this.getParams(data) + '/' : '/';
    urlFinal = this.getName() + urlStr + "/" + urlId;

    const object = {
      method,
      url: urlFinal,
      headers: {},
      data: data,
      params: method === "GET" ? data || [] : null,
    };
    const local = sessionStorage.getItem("token");

    let headers: any = {
      ...providedHeaders,
      "Content-Type": "application/json",
      "Access-Control-Allow-Origin": "*",
      Accept: "application/json",
    };

    const headerParent = this.getHeaders();
    if (headerParent) {
      object.headers = Object.assign(headers, headerParent);
    }
    object.headers = headers;
    return object;
  }
}
