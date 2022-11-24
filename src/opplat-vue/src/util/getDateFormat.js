
export default {
    getDateTime(date) {
        const fecha = date.split("T")[0];
        const hour = date.split(":")[0];
        const minSec = date.split(":")[1].split(":")[0];
        const number = +hour - 12;
        if (+hour === 0) {
            return 12 + ":" + minSec + "am";
        }
        if (number < 0) {
            return hour + ":" + minSec + "am";
        }
        return !!+number
            ? fecha + " " + number + ":" + minSec + "pm"
            : fecha + " " + 12 + ":" + minSec + "pm";
    },
};
