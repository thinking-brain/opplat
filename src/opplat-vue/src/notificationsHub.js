import { HubConnectionBuilder, LogLevel } from '@aspnet/signalr';

export default {
  install(Vue) {
    const connection = new HubConnectionBuilder()
      .withUrl('https://localhost:5001/notiHub')
      .configureLogging(LogLevel.Information)
      .build();
    connection.start().catch(err => console.log(err));
  },
};
