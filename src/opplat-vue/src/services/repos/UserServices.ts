import AxiosConfigClass from "@/services/axios/AxiosConfigClass";
import BaseRepository from "@/services/repos/BaseRepository";
import { RouteOptions } from "@/services/repos/IRepository";

interface IUserService {
  [propName: string]: any;

  address(options: RouteOptions);
  confirm(options: RouteOptions);
  nonValid(options: RouteOptions);
}

interface IService extends BaseRepository, IUserService {}

const config = new AxiosConfigClass();
config.setAxiosInstance(process.env.VUE_APP_AUTH_URL);
config.setName("users");
config.urlList({});

class UserServices extends BaseRepository {}

export default new UserServices(config) as IService;
