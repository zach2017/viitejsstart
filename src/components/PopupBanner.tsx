import { useState } from 'react';

const PopupBanner = () => {
  const [visible, setVisible] = useState(true);

  if (!visible) return null;

  return (
    <div style={{
      position: 'fixed', top: 0, left: 0, width: '100%',
      background: 'red', color: 'white', padding: '10px', textAlign: 'center'
    }}>
      <p>Warning: This is a demo site.</p>
      <button onClick={() => setVisible(false)}>Close</button>
    </div>
  );
};

export default PopupBanner;
