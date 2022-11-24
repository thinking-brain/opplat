import { register } from "register-service-worker";

interface SimpleNotificationObject {
  body: any;
  header: string;
}

if (process.env.NODE_ENV === "production") {
  register(`${process.env.VUE_APP_BASE_URL}/service-worker.js`, {
    ready() {
      self.addEventListener("push", (e: any) => {
        let data: SimpleNotificationObject = {} as SimpleNotificationObject;
        if ("data" in e) {
          data = e.data.json();
        }
        const options = {
          body: data.body,
          header: data.header,
          // icon: '/img/icons/amdroid-chrome-192x192.png',
          image: "",
          vibrate: [300, 200, 300],
          badge: "",
        };
        // @ts-ignore
        e.waitUntil(self.registration.showNotification(data.header, options));
      });
    },
  });
}
