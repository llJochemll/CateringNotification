enum Campus {
    "Diepenbeek",
    "ElfdeLinie",
    "Vildersstraat"
}

enum Day {
    monday = "monday",
    tuesday = "tuesday",
    wednesday = "wednesday",
    thursday = "thursday",
    friday = "friday"
}

class NotificationTime {
    Hour: number;
    Minute: number;
}

class CampusTime { 
    Key: Campus; 
    Value: NotificationTime;
}

class Subscription {
    NotificationSettings = new Map<Day, CampusTime>();
}

const buildSubscription = (): Subscription => {
    const subscription = new Subscription();

    Object.keys(Day).forEach((day: string) => {
        if ((<HTMLInputElement>document.getElementById("no_" + day)).checked === false) {
            subscription.NotificationSettings[day] = new CampusTime();

            subscription.NotificationSettings[day].Key = (<HTMLSelectElement>document.getElementById(day + "_location")).selectedOptions[0].value;
            subscription.NotificationSettings[day].Value = getNotificationTime(day);
        }
    });

    return subscription;
}

const getNotificationTime = (day: string): NotificationTime => {
    let notificationTime = new NotificationTime();
    let time = (<HTMLInputElement>document.getElementById(day)).value.split(":");
    notificationTime.Hour = +time[0];
    notificationTime.Minute = +time[1];

    return notificationTime;
}

const subscribe = async (): Promise<void> => {
    const email = (<HTMLInputElement>document.getElementById("email")).value;

    const subscription = buildSubscription();

    const response = await fetch(`https://pxlcateringnotification.azurewebsites.net/api/subscribe/${email}`, {
        body: JSON.stringify(subscription),
        headers: {
            "Content-Type": "application/json"
        },
        method: "POST",
    });

    if (response.status != 200) {
        alert(`Oeps, er trad een error op! Weet je zeker dat alle gegevens correct zijn?`);
    } else {
        window.location.href = `https://cateringstorage.z6.web.core.windows.net/verify?email=${email}`;
    }
}

const verify = async (): Promise<void> => {
    const email = (<HTMLInputElement>document.getElementById("email")).value;
    const token = (<HTMLInputElement>document.getElementById("token")).value;

    const response = await fetch(`https://pxlcateringnotification.azurewebsites.net/api/verify/${email}/${token}`, {
        headers: {
            "Content-Type": "application/json"
        },
        method: "POST",
    });

    if (response.status != 200) {
        alert(`Oeps, er trad een error op! Weet je zeker dat je email en token correct zijn?`);
    } else {
        alert(`Je bevestiging is succesvol doorgegeven!`);
    }
}

const getEmailFromURL = (): void => {
    let url = new URL(window.location.href);
    let email = url.searchParams.get("email");
    (<HTMLInputElement>document.getElementById("email")).value = email;
}

const checkTokenInput = (): void => {
    const expression = /\S+@\S+/;
    if ((<HTMLInputElement>document.getElementById("token")).value.length === 6 && expression.test(String((<HTMLInputElement>document.getElementById("email")).value).toLowerCase())) {
        (<HTMLInputElement>document.getElementById("submit")).disabled = false;
    } else {
        (<HTMLInputElement>document.getElementById("submit")).disabled = true;
    }
}

const checkEmailInput = (): void => {
    const expression = /\S+@\S+/;
    if (expression.test(String((<HTMLInputElement>document.getElementById("email")).value).toLowerCase())) {
        (<HTMLInputElement>document.getElementById("submit")).disabled = false;
    } else {
        (<HTMLInputElement>document.getElementById("submit")).disabled = true;
    }
}

const no_email_clicked = (day: string): void => {
    const timeElement = (<HTMLInputElement>document.getElementById(day));
    const locationElement = (<HTMLSelectElement>document.getElementById(day + "_location"));
    console.log(timeElement);
    console.log(locationElement);
    if ((<HTMLInputElement>document.getElementById("no_" + day)).checked === true) {
        timeElement.value = "--:--";
        timeElement.disabled = true;
        locationElement.value = "";
        locationElement.disabled = true;
    } else {
        timeElement.value = "11:45";
        timeElement.disabled = false;
        locationElement.selectedIndex = 0;
        locationElement.disabled = false;
    }
}