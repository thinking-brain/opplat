import BaseRepository from "@/services/repos/BaseRepository";
import AxiosConfigClass from "@/services/axios/AxiosConfigClass";
interface IStoresService {
  [propName: string]: any;
}

interface IService extends BaseRepository, IStoresService {}

const config = new AxiosConfigClass();
config.setName("products");
config.urlList({});

class ProductsService extends BaseRepository {}

export default new ProductsService(config) as IService;
