import { useEffect, useState } from "react";
import {
    deleteAchievement,
    getAchievements,
} from "../../api/achievementApi";
import type { Achievement } from "../../types/achievement";
import AchievementCard from "./AchievementCard";
import AchievementModal from "./AchievementModal";
import { getCurrentUser } from "../../api/LoginApi";
import { getUserAchievements } from "../../api/UserApi";

interface Props {
    userId?: string;
    refreshTrigger: number;
}

export default function AchievementList({
    refreshTrigger,
    userId
}: Props) {
    const [achievements, setAchievements] = useState<Achievement[]>([]);
    const [loading, setLoading] = useState(true);
    const [currentUserId, setCurrentUserId] = useState<string | null>(null);

    const [isEditModalOpen, setIsEditModalOpen] = useState(false);
    const [selectedAchievement, setSelectedAchievement] = useState<Achievement | undefined>();

    const loadAchievements = async () => {
        try {
            setLoading(true);

            const data = userId
                ? await getUserAchievements(userId)
                : await getAchievements();

            setAchievements(data);
        } catch (error) {
            console.error(error);
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        loadAchievements();

        getCurrentUser()
            .then((user) => setCurrentUserId(user.userId))
            .catch((error) => console.error("Failed to fetch current user:", error));
    }, [refreshTrigger, userId]);

    const handleDelete = async (id: string) => {
        try {
            await deleteAchievement(id);

            await loadAchievements();
        } catch (error) {
            console.error(error);
            alert("Failed to delete achievement");
        }
    };

    const handleEdit = (achievement: Achievement) => {
        setSelectedAchievement(achievement);

        setIsEditModalOpen(true);
    };

    if (loading) {
        return <p>Loading achievements...</p>;
    }
    
    return (
        <div>
            <h2>Achievements</h2>

            <AchievementModal 
                open={isEditModalOpen}
                achievement={selectedAchievement}
                onClose={() => {
                    setIsEditModalOpen(false);

                    setSelectedAchievement(undefined);
                }}
                onAchievementCreated={loadAchievements}
            />

            {achievements.length === 0 && <p>No achievements yet.</p>}

            {achievements.map((achievement) => (
                <div key={achievement.achievementId}>
                    <AchievementCard 
                        achievement={achievement} 
                        currentUserId={currentUserId}
                    />

                    <button 
                        onClick={() => handleEdit(achievement)}
                    >
                        Edit
                    </button>
                    <button 
                        onClick={() => handleDelete(achievement.achievementId)}
                    >
                        Delete
                    </button>
                </div>
            ))}
        </div>
    );
}