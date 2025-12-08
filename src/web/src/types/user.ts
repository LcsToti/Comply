export type User = {
    id: string;
    name: string;
    email: string;
    phoneNumber?: string;
    profilePic: string;
    role: number;
    createdAt: string;
}

export type UserAddress = {
    id: string;
    street: string;
    number: string;
    city: string;
    state: string;
    zipCode: string;
}