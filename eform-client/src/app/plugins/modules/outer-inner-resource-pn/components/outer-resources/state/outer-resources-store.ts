import { Injectable } from '@angular/core';
import { Store, StoreConfig } from '@datorama/akita';

export interface OuterResourcesState {
  pageSize: number;
  sort: string;
  isSortDsc: boolean;
  offset: number;
}

export function createInitialState(): OuterResourcesState {
  return {
    pageSize: 10,
    sort: 'Id',
    isSortDsc: false,
    offset: 0,
  };
}

@Injectable({ providedIn: 'root' })
@StoreConfig({ name: 'outerResources' })
export class OuterResourcesStore extends Store<OuterResourcesState> {
  constructor() {
    super(createInitialState());
  }
}
