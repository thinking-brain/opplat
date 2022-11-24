import AxiosConfigClass from "../axios/AxiosConfigClass";
import { IRepository, WSMethod } from "@/services/repos/IRepository";
import { Method } from "axios";

export default class BaseRepository implements IRepository {
  [propName: string]: any;

  public readonly provider: AxiosConfigClass;
  public readonly url: object;
  public readonly defaultMethod: string[] = ["list"];

  constructor(obj) {
    this.provider = obj;
    this.url = obj.getUrlList();
    this.setMethods();
  }

  public async makeRequest(
    method: Method | WSMethod,
    data?: any,
    url?: string,
    id?: string | number,
    providedHeaders = {} as { [name: string]: any }
  ) {
    return await this.provider.makeRequest(
      method,
      data,
      url,
      id,
      providedHeaders
    );
  }

  public checkUrl(url: string): boolean {
    if (this.defaultMethod.includes(url)) {
      throw new Error("This Url is predefined, please use another name");
    }

    if (!this.url.hasOwnProperty(url)) {
      throw new Error("You must config this Url before use this methods");
    }
    return true;
  }

  // Métodos Predefinidos
  public async get({
    method = "GET" as Method | WSMethod,
    id = "" as string | number,
    url = "",
    providedHeaders = {} as { [name: string]: any },
  }) {
    return await this.makeRequest(method, {}, url, id, providedHeaders);
  }

  public async list({
    method = "GET" as Method | WSMethod,
    data = {} as object,
    url = "" as string,
    id = "" as number | string,
    providedHeaders = {} as { [name: string]: any },
  }) {
    return await this.makeRequest(method, data, url, id, providedHeaders);
  }

  public async delete({
    method = "DELETE" as Method | WSMethod,
    id = "" as number | string,
    url = "",
    providedHeaders = {} as { [name: string]: any },
  }) {
    return await this.makeRequest(method, {}, url, id, providedHeaders);
  }

  public async deleteAll({
    method = "GET" as Method | WSMethod,
    data = {} as any,
    url = "" as string,
    id = "" as number | string,
    providedHeaders = {} as { [name: string]: any },
  }) {
    return await this.makeRequest(method, data, url, id, providedHeaders);
  }

  public async insert({
    method = "POST" as Method | WSMethod,
    data = {} as object,
    url = "" as string,
    id = "" as number | string,
    providedHeaders = {} as { [name: string]: any },
  }) {
    return await this.makeRequest(method, data, url, id, providedHeaders);
  }

  public async update({
    method = "PUT" as Method | WSMethod,
    data = {} as object,
    url = "",
    id = "" as number | string,
    providedHeaders = {} as { [name: string]: any },
  }) {
    return await this.makeRequest(method, data, url, id, providedHeaders);
  }

  private setMethods(): void {
    for (const internalMethod of Object.keys(this.url)) {
      this[internalMethod] = async ({
        method = "POST" as Method | WSMethod,
        data = {} as object,
        id = "" as number | string,
        url = "",
        providedHeaders = {} as { [name: string]: any },
      }) => {
        if (this.checkUrl(internalMethod)) {
          return await this.makeRequest(method, data, url, id, providedHeaders);
        }
      };
    }
  }
}
