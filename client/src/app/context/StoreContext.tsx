import { PropsWithChildren, createContext, useContext, useState } from "react";
import { Basket } from "../models/Basket";

// 所有應用程序都可以使用這邊的屬性&方法
interface StoreContextValue {
    // 存放籃子
    basket: Basket | null;
    setBasket: (basket : Basket) => void;
    // 刪除item後，需改變前端basket內商品數量
    removeItem: (productId: number, quantity: number) => void;
}

export const StoreContext = createContext<StoreContextValue | undefined>(undefined);

export function useStoreContext(){
    const context = useContext(StoreContext);

    if(context === undefined){
        throw Error("Oops - we do not seem to be inside the provider")
    }

    return context;
}

export function StoreProvider({children}: PropsWithChildren<unknown>){
    const [basket, setBasket] = useState<Basket | null>(null);

    function removeItem (productId: number, quantity: number){
        if (!basket) return;
        const items = [...basket.items];
        const itemIndex = items.findIndex(i => i.productId === productId);
        if (itemIndex >= 0){
            items[itemIndex].quantity -= quantity;
            if (items[itemIndex].quantity === 0) items.splice(itemIndex, 1);
            setBasket(prevState => {
                return{...prevState!, items}
            })
        }
    }

    return(
        <StoreContext.Provider value={{basket, setBasket, removeItem}}>
            {children}
        </StoreContext.Provider>
    )
}