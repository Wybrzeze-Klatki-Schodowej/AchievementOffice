export interface UserRanking {
    userId: string;
    userName: string;
    firstname: string;
    lastname: string;
    avatar: string | null;
    rankingPoints: number;
}

export interface GroupRanking {
    groupId: string;
    groupName: string;
    avatar: string | null;
    rankingPoints: number;
}