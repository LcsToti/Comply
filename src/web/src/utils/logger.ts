// src/utils/logger.ts

function formatDate(): string {
    const now = new Date();
    const pad = (n: number) => n.toString().padStart(2, '0');
    const ms = now.getMilliseconds().toString().padStart(3, '0');
    return `${now.getFullYear()}-${pad(now.getMonth() + 1)}-${pad(now.getDate())} ` +
           `${pad(now.getHours())}:${pad(now.getMinutes())}:${pad(now.getSeconds())}.${ms}`;
}

export const logger = {
    log: (...args: any[]) => {
        console.log(`[${formatDate()}]`, ...args);
    },
    warn: (...args: any[]) => {
        console.warn(`[${formatDate()}]`, ...args);
    },
    error: (...args: any[]) => {
        console.error(`[${formatDate()}]`, ...args);
    },
};
