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
    visibilityId: number; // 1=Public, 2=Private, 3=Group
    groupIds: string[];
    createdAt: string;
    updatedAt: string;
    reactions: Record<string, number>;
    currentUserReaction?: string;
}

export interface User {
    userId: string;
    login: string;
}
