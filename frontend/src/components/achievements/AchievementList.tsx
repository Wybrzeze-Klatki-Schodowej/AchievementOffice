import { useEffect, useState } from "react";
import {
    deleteAchievement,
    getAchievements,
} from "../../api/achievementApi";
import type { Achievement } from "../../types/achievement";
import AchievementCard from "./AchievementCard";
import AchievementModal from "./AchievementModal";

interface Props {
    refreshTrigger: number;
}

export default function AchievementList({
    refreshTrigger,
}: Props) {
    const [achievements, setAchievements] = useState<Achievement[]>([]);
    const [loading, setLoading] = useState(true);

    const [isEditModalOpen, setIsEditModalOpen] = useState(false);
    const [selectedAchievement, setSelectedAchievement] = useState<Achievement | undefined>();

    const loadAchievements = async () => {
        try {
            setLoading(true);

            const data = await getAchievements();

            setAchievements(data);
        } catch (error) {
            console.error(error);
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        loadAchievements();
    }, [refreshTrigger]);

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
                    <AchievementCard achievement={achievement} />

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