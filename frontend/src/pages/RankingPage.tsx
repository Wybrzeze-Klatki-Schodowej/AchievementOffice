import "./RankingPage.css";
import { useState } from 'react';
import UsersTable from "../components/rankings/UsersTable";
import GroupsTable from "../components/rankings/GroupsTable";

type ViewState = 'Users' | 'Groups'

export default function RankingPage() {
    const [currentView, setCurrentView] = useState<ViewState>('Users');

    return (<div className="ranking-wrapper">
        <div className="ranking-header">
            <select className="view-selector" value={currentView} onChange={(e) => setCurrentView(e.target.value as ViewState)}>
                <option value="Users">Users ranking</option>
                <option value="Groups">Groups ranking</option>
            </select>
        </div>
        {currentView === 'Users' ? <UsersTable/> : <GroupsTable/>}
    </div>);
}