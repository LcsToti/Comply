import { useState, useEffect } from "react";

const getRemainingTime = (targetDate: string | null | undefined): string => {
    if (!targetDate) return "N/A";

    const endTime = new Date(targetDate).getTime();
    const now = new Date().getTime();
    const diff = endTime - now;

    if (diff <= 0) {
        return "00:00:00";
    }

    const seconds = Math.floor((diff / 1000) % 60);
    const minutes = Math.floor((diff / 1000 / 60) % 60);
    const hours = Math.floor((diff / (1000 * 60 * 60)) % 24);
    const days = Math.floor(diff / (1000 * 60 * 60 * 24));

    let timeString = "";
    if (days > 0) timeString += `${days}d `;
    if (hours > 0 || days > 0) timeString += `${hours.toString().padStart(2, "0")}h `;
    timeString += `${minutes.toString().padStart(2, "0")}m `;
    timeString += `${seconds.toString().padStart(2, "0")}s`;

    return timeString.trim();
};

export const useCountdown = (
    targetDate: string | null | undefined,
    onEnd?: () => void
) => {
    const [remainingTime, setRemainingTime] = useState(() =>
        getRemainingTime(targetDate)
    );

    useEffect(() => {
        if (!targetDate) {
            setRemainingTime("N/A");
            return;
        }

        const interval = setInterval(() => {
            const newTime = getRemainingTime(targetDate);
            setRemainingTime(newTime);

            if (newTime === "00:00:00") {
                clearInterval(interval);
                if (onEnd) onEnd();
            }
        }, 1000);

        return () => clearInterval(interval);
    }, [targetDate, onEnd]);

    return remainingTime;
};