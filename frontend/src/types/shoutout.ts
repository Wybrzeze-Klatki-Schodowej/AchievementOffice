export interface Shoutout {
    shoutoutId: string;
    senderId: string;
    senderLogin: string;
    senderFirstname: string;
    senderLastname: string;
    receiverId: string;
    receiverLogin: string;
    receiverFirstname: string;
    receiverLastname: string;
    title: string;
    description?: string;
    createdAt: string;
    updatedAt: string;
    // deletedAt?: string;
}

export interface User {
    userId: string;
    login: string;
}
