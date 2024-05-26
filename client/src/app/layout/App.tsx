import { Outlet } from "react-router-dom";
import Header from "../layout/Header";
import { Container, CssBaseline, ThemeProvider, createTheme } from "@mui/material";
import { useEffect, useState } from "react";
import { ToastContainer } from "react-toastify";
import 'react-toastify/dist/ReactToastify.css';
import { useStoreContext } from "../context/StoreContext";
import { getCookie } from "../util/util";
import agent from "../api/agent";
import LoadingComponent from "./LoadingComponent";

function App() {
  const {setBasket} = useStoreContext();
  const [loading, setLoading] = useState(true);
  
  useEffect(() => {
    const buyerId = '068ecc90-1085-4de3-9b4a-1f73d70baaca'
    console.log(buyerId)
    if(buyerId){
      agent.Basket.get()
        .then(basket => setBasket(basket))
        .catch(error => console.log(error))
        .finally(() => setLoading(false));
    } else {
      setLoading(false);
    }
  }, [setBasket])
  
  const [darkMode, setDarkMode] = useState(false);
  const paletteType = darkMode ? 'dark': 'light';
  const theme = createTheme({
    palette:{
      mode: paletteType,
      background: {
        default: paletteType === 'light' ? '#eaeaea' : '#121212'
      }
    }
  })

  function handleThemeChange() {
    setDarkMode(!darkMode);
  }

  if(loading) return <LoadingComponent message="Initialising app..."/>

  return (
    <ThemeProvider theme={theme}>
      <ToastContainer position="bottom-right" hideProgressBar theme="colored" />
      <CssBaseline />
      <Header handleThemeChange={handleThemeChange} darkMode={darkMode}/>
      <Container>
        <Outlet />
      </Container>
    </ThemeProvider>
  )
}

export default App
