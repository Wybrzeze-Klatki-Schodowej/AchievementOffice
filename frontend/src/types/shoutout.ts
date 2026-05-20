export interface Shoutout {
    shoutoutId: string;
    receiverId: string;
    senderId: string;
    title: string;
    description?: string;
    createdAt: string;
    updatedAt: string;
    deletedAt?: string;
    visibilityId: number;
    categoryId: number;
}

export interface User {
    userId: string;
    login: string;
}