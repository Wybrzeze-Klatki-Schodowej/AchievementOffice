import { useState, useEffect } from "react";
import { getGroupRanking } from "../../api/RankingApi"
import { type GroupRanking } from "../../types/RankingTypes"
import "./RankingTable.css"

export default function GroupsTable() {
    const [groupList, setGroupList] = useState<GroupRanking[]>([]);
    
        const loadList = async () => {
            const groupList = await getGroupRanking();
            setGroupList(groupList);
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
                        <th style={{ width: "100%" }}>Group name</th>
                        <th style={{ textAlign: "right", width: "120px", paddingRight: "40px" }} >Points</th>
                    </tr>
                </thead>
                <tbody>
                    {groupList.map((group, index) => (
                        <tr key={group.groupId}>
                            <td>{index + 1}</td>
                            <td>{group.avatar ? <img src={group.avatar} width="30" height="30" style={{ borderRadius: '50%' }}/> : <div className="placeholder-avatar">👤</div>}</td>
                            <td style={{ width: "100%" }}><strong>{group.groupName}</strong></td>
                            <td style={{ textAlign: "right", paddingRight: "40px" }}>{group.rankingPoints}</td>
                        </tr>
                    ))}
                </tbody>
            </table>
    
            {groupList.length === 0 && <p>No data to show</p>}
        </div>);
}