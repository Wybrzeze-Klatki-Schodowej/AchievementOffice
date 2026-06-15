import { useState, useEffect } from "react";
import { getUserRanking } from "../../api/RankingApi"
import { type UserRanking } from "../../types/RankingTypes"
import "./RankingTable.css"

export default function UsersTable() {
    const [usersList, setUsersList] = useState<UserRanking[]>([]);

    const loadList = async () => {
        const userList = await getUserRanking();
        setUsersList(userList);
    }

    useEffect(() => {
        loadList();
    }, []);

    return (<div className="ranking-container">
        <table className="ranking-table">
            <thead>
                <tr>
                    <th>Place</th>
                    <th>Avatar</th>
                    <th>User</th>
                    <th>Full name</th>
                    <th>Points</th>
                </tr>
            </thead>
            <tbody>
                {usersList.map((user, index) => (
                    <tr key={user.userId}>
                        <td>{index + 1}</td>
                        <td>{user.avatar ? <img src={user.avatar} width="30" height="30" style={{ borderRadius: '50%' }}/> : <div className="placeholder-avatar">👤</div>}</td>
                        <td><strong>{user.userName}</strong></td>
                        <td>{user.firstname} {user.lastname}</td>
                        <td>{user.rankingPoints}</td>
                    </tr>
                ))}
            </tbody>
        </table>

        {usersList.length === 0 && <p>No data to show</p>}
    </div>);
}