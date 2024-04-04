import React, { useEffect, useState } from 'react';
import { CFormInput, CButton, CCard, CCardBody, CCardHeader } from '@coreui/react';
import '@coreui/coreui/dist/css/coreui.min.css';

function App() {
  const [messages, setMessages] = useState([]);
  const [inputValue, setInputValue] = useState('');
  const [ws, setWs] = useState(null);

  useEffect(() => {
    const newWs = new WebSocket('ws://localhost:5281/ws');

    newWs.onmessage = (event) => {
      // Directamente maneja el mensaje como texto en lugar de parsearlo como JSON
      const message = event.data;
      setMessages((prevMessages) => [...prevMessages, message]);
    };

    setWs(newWs);

    return () => {
      newWs.close();
    };
  }, []);

  const sendMessage = () => {
    if (ws && inputValue.trim()) {
      ws.send(JSON.stringify(inputValue));
      setInputValue('');
    }
  };

  return (
    <div className='App'>
      <CCard>
        <CCardHeader>Mensajes</CCardHeader>
        <CCardBody>
          {messages.map((msg, index) => (
            <div key={index}>{msg}</div>
          ))}
          <CFormInput
            type='text'
            value={inputValue}
            onChange={(e) => setInputValue(e.target.value)}
            onKeyPress={(e) => e.key === 'Enter' && sendMessage()}
            placeholder='Escribe un mensaje...'
          />
          <CButton color='primary' onClick={sendMessage}>
            Enviar
          </CButton>
        </CCardBody>
      </CCard>
    </div>
  );
}

export default App;
