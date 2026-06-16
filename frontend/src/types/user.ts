export interface UserProfile {
    userId: string;

    login: string;

    email: string;

    firstName: string;

    lastName: string;

    jobTitle: string;

    isActive: boolean;

    bio?: string;

    avatarUrl?: string;

    role: string;

    createdAt: string;

    updatedAt: string;

    visibilityId: number; // 1=Public, 2=Private, 3=Group
    groupIds: string[];
    isProfileRestricted: boolean;
}