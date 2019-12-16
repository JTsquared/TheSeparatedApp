//function getEndpoint() {
//    return self.registration.pushManager.getSubscription()
//        .then(function (subscription) {
//            if (subscription) {
//                return subscription.endpoint;
//            }

//            throw new Error('User not subscribed');
//        });
//}

//self.addEventListener('push', function (event) {
//    event.waitUntil(
//        getEndpoint()
//            .then(function (endpoint) {
//                return fetch('./getPayload?endpoint=' + endpoint);
//            })
//            .then(function (response) {
//                return response.text();
//            })
//            .then(function (payload) {
//                self.registration.showNotification('ServiceWorker Cookbook', {
//                    body: payload,
//                });
//            })
//    );
//});

self.addEventListener("push", event => {
    if (event.data) {
        var payload = event.data.json();
        var title = payload.Title;
        //var title = payload.Model.DependentName;
        var icon = '/images/android-chrome-192x192.png';
        var tag = 'simple-push-demo-notification-tag';
        //var tag = payload.Data;
        var options = {
            body: payload.Message,
            icon: icon,
            tag: tag,
            data: payload.Data
        };

        event.waitUntil(self.registration.showNotification(title, options));
    };
});

//self.addEventListener("notificationclick", function (event) {
//    var notification = event.notification;
//    var primaryKey = notification.data.primaryKey;
//    var action = event.action;

//    if (action == 'close') {
//        notification.close();
//    }
//    else {
//        event.waitUntil(clients.openWindow('/ReportMissing/ReportMatch'));
//    }
//    console.log("it worked?");
//});

self.addEventListener("notificationclick", function (event){
    //event.waitUntil(alert(event.notification));
    //var url = "/ReportMissing/ReportMatch/" + event.notification.data.tag + "";
    //var url = "/ReportMissing/ReportMatch";
    //var strJSON = encodeURIComponent(JSON.stringify(event.notification.data));
    var strJSON = encodeURIComponent(event.notification.data);
    var url = "/ReportMissing/ReportMatch?notificationTag=" + strJSON;
    return clients.openWindow(url);
});

//self.addEventListener('push', function (event) {
//    console.log('Received a push message', event);

//    var title = 'We found a match!';
//    var body = 'Click here to view the details';
//    var icon = '/images/android-chrome-192x192.png';
//    var tag = 'simple-push-demo-notification-tag';

//    event.waitUntil(
//        self.registration.showNotification(title, {
//            body: body,
//            icon: icon,
//            tag: tag
//        })
//    );
//});
