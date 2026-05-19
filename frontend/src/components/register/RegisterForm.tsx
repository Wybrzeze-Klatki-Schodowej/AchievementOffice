import React, { useState } from 'react';
import type { RegisterDTO } from '../../api/RegisterApi';
import '../login/LoginForm.css';
import { Link } from 'react-router-dom';

interface RegisterFormProps {
    isLoading: boolean;
    onSubmit: (data: RegisterDTO) => void;
    error: string | null;
}

export default function RegisterForm({ isLoading, onSubmit, error }: RegisterFormProps) {
    const [email, setEmail] = useState('');
    const [username, setUsername] = useState('');
    const [password, setPassword] = useState('');
    const [firstname, setFirstname] = useState('');
    const [lastname, setLastname] = useState('');
    const [jobTitle, setJobTitle] = useState('');

    const handleSubmit = (e: React.FormEvent) => {
        e.preventDefault();
        onSubmit({ 
            Email: email, 
            Username: username, 
            Password: password, 
            Firstname: firstname, 
            Lastname: lastname, 
            JobTitle: jobTitle,
            RoleName: 'User'
        });
    };

    return (
        <form onSubmit={handleSubmit}>
            <h2>Rejestracja</h2>
            <input type="email" placeholder='Email' value={email} onChange={(e) => setEmail(e.target.value)} required />
            <input type="text" placeholder='Nazwa użytkownika' value={username} onChange={(e) => setUsername(e.target.value)} required />
            <input type="password" placeholder='Hasło (min. 8 znaków)' value={password} onChange={(e) => setPassword(e.target.value)} minLength={8} required />
            <input type="text" placeholder='Imię' value={firstname} onChange={(e) => setFirstname(e.target.value)} required />
            <input type="text" placeholder='Nazwisko' value={lastname} onChange={(e) => setLastname(e.target.value)} required />
            <input type="text" placeholder='Stanowisko' value={jobTitle} onChange={(e) => setJobTitle(e.target.value)} required />
            
            {error && <div style={{color: 'red', margin: '1rem 0'}} className="error-message">{error}</div>}
            
            <button className="login-button" type="submit" disabled={isLoading}>
                {isLoading ? 'Ładowanie...' : 'Zarejestruj'}
            </button>

            <div style={{marginTop: '1rem', fontSize: '0.9rem'}}>
                Masz już konto? <Link to="/login">Zaloguj się</Link>
            </div>
        </form>
    );
}