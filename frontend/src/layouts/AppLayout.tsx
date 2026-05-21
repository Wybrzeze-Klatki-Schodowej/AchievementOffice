import { useState } from "react";
import { Outlet } from "react-router-dom";
import Navbar from "../components/layout/Navbar";
import AchievementModal from "../components/achievements/AchievementModal";
import ShoutoutModal from "../components/shoutouts/ShoutoutModal";
import UserList from "../components/users/UserList";

export default function AppLayout() {
    const [isAchievementModalOpen, setIsAchievementModalOpen] = useState(false);
    const [isShoutoutModalOpen, setIsShoutoutModalOpen] = useState(false);

    const [refreshTrigger, setRefreshTrigger] = useState(0);

    const handleAchievementCreated = () => {
        setRefreshTrigger((prev) => prev + 1);
    };

    const handleShoutoutCreated = () => {
        setRefreshTrigger((prev) => prev + 1);
    };

    return (
        <>
            <Navbar 
                onAddAchievementClick={() =>
                    setIsAchievementModalOpen(true)
                }

                onAddShoutoutClick={() =>
                    setIsShoutoutModalOpen(true)
                }
            />


            <AchievementModal 
                open={isAchievementModalOpen}
                onClose={() => setIsAchievementModalOpen(false)}
                onAchievementCreated={handleAchievementCreated}
            />

            <ShoutoutModal 
                open={isShoutoutModalOpen}
                onClose={() => setIsShoutoutModalOpen(false)}
                onShoutoutCreated={handleShoutoutCreated}
            />

            <div 
                style={{ 
                    display: "flex",
                    height: "calc(100vh - 60px)",
                    overflow: "hidden",
                }}
            >
                <aside
                    style={{
                        width: "250px",
                        borderRight: "1px solid #ddd",
                        height: "calc(100vh - 60px)",
                        overflowY: "auto",
                    }}
                >
                    <UserList />
                </aside>

                <main 
                    style={{ 
                        flex: 1,
                        overflowY: "auto",
                        minHeight: "0",
                        padding: "24px",
                    }}
                >
                    <Outlet 
                        context={{ refreshTrigger }}
                    />
                </main>
            </div>
        </>
    );
}