import LoginForm from '../components/login/LoginForm';
import { useState } from 'react';
import type { LoginDTO, RegisterDTO } from '../api/LoginApi';
import { login, register } from '../api/LoginApi';
import { useNavigate } from 'react-router-dom';
import './LoginPage.css';

interface LoginPageProps {
    onLogin: () => void;
}

export default function LoginPage({ onLogin }: LoginPageProps) {
    const [isLoading, setIsLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);
    const [successMessage, setSuccessMessage] = useState<string | null>(null);
    const [mode, setMode] = useState<'login' | 'register'>('login');
    const navigate = useNavigate();

    const handleSubmit = async (data: LoginDTO | RegisterDTO) => {
        setIsLoading(true);
        setError(null);
        setSuccessMessage(null);

        try {
            if (mode === 'login') {
                await login(data as LoginDTO);
                onLogin();
                navigate('/');
            } else {
                await register(data as RegisterDTO);
                setSuccessMessage('Registration successful. Please log in.');
                setMode('login');
            }
        } catch (err: any) {
            setError(err.message);
        } finally {
            setIsLoading(false);
        }
    };

    const toggleMode = () => {
        setError(null);
        setSuccessMessage(null);
        setMode((prevMode) => (prevMode === 'login' ? 'register' : 'login'));
    };

    return (
        <div className='login-page'>
            <div className='text-container'>
                <h1 className='top-left-text'>Share your success</h1>
            </div>
            <div className='form-container'>
                <h2 className='login-title'>{mode === 'login' ? 'Login' : 'Register'}</h2>
                <LoginForm
                    isLoading={isLoading}
                    mode={mode}
                    onSubmit={handleSubmit}
                    onModeChange={toggleMode}
                    error={error}
                    successMessage={successMessage}
                />
            </div>
        </div>
    );
}