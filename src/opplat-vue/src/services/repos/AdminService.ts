import BaseRepository from "@/services/repos/BaseRepository";
import AxiosConfigClass from "@/services/axios/AxiosConfigClass";
interface IAdminService {
  [propName: string]: any;
}

interface IService extends BaseRepository, IAdminService {}

const config = new AxiosConfigClass();
config.setName("admin");
config.urlList({});

class AdminService extends BaseRepository {}

export default new AdminService(config) as IService;
