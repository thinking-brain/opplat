import BaseRepository from "@/services/repos/BaseRepository";
import AxiosConfigClass from "@/services/axios/AxiosConfigClass";
import { RouteOptions } from "@/services/repos/IRepository";

interface IAuthService {
  login(options: RouteOptions);
  register(options: RouteOptions);
  logout(options: RouteOptions);
  getRoles(options: RouteOptions);
}

interface IService extends BaseRepository, IAuthService {}

const config = new AxiosConfigClass();
config.setName("auth");
config.urlList({
  login: "login/",
  logout: "logout",
  register: "signup",
  getRoles: "getRoles",
});

class AuthServices extends BaseRepository {}

export default new AuthServices(config) as IService;
