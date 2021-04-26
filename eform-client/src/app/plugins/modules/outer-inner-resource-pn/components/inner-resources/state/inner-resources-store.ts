import { Injectable } from '@angular/core';
import { Store, StoreConfig } from '@datorama/akita';

export interface InnerResourcesState {
  pageSize: number;
  sort: string;
  isSortDsc: boolean;
  offset: number;
}

export function createInitialState(): InnerResourcesState {
  return {
    pageSize: 10,
    sort: 'Id',
    isSortDsc: false,
    offset: 0,
  };
}

@Injectable({ providedIn: 'root' })
@StoreConfig({ name: 'innerResources' })
export class InnerResourcesStore extends Store<InnerResourcesState> {
  constructor() {
    super(createInitialState());
  }
}
