import "./RankingPage.css";
import { useState } from 'react';
import UsersTable from "../components/rankings/UsersTable";
import GroupsTable from "../components/rankings/GroupsTable";

type ViewState = 'Users' | 'Groups'

export default function RankingPage() {
    const [currentView, setCurrentView] = useState<ViewState>('Users');

    return (<div>
        <select value={currentView} onChange={(e) => setCurrentView(e.target.value as ViewState)}>
            <option value="Users">Users ranking</option>
            <option value="Groups">Groups ranking</option>
        </select>

        {currentView === 'Users' ? <UsersTable/> : <GroupsTable/>}
    </div>);
}