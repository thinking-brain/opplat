import { Method } from "axios";

export type WSMethod = "ws" | "WS";

export interface RouteOptions {
  method?: Method | WSMethod;
  url?: string;
  data?: any;
  id?: string | number;
  providedHeaders?: { [name: string]: any };
}

export interface IRepository {
  [propName: string]: any;

  readonly provider;
  readonly url: object;
  readonly defaultMethod: string[];

  makeRequest(
    method: Method | WSMethod,
    data?: JSON,
    url?: string,
    id?: number | string,
    providedHeader?: { [name: string]: any }
  );

  get(options: RouteOptions);

  list(options: RouteOptions);

  insert(options: RouteOptions);

  update(options: RouteOptions);

  delete(options: RouteOptions);

  deleteAll(options: RouteOptions);

  checkUrl(url: string): boolean;
}
